using System;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace DeviceMonitor;

public partial class Form1 : Form
{
    private DeviceDataManager _dataManager;
    private TabControl _tabControl = null!;
    private TabPage _tabSummary = null!;
    private TabPage _tabHistory = null!;
    private DataGridView _gridSummary = null!;
    private DataGridView _gridHistory = null!;
    private BindingSource _summaryBindingSource = null!;
    private BindingSource _historyBindingSource = null!;
    private ManagementEventWatcher _insertWatcher = null!;
    private ManagementEventWatcher _removeWatcher = null!;

    public Form1()
    {
        InitializeComponent();
        this.Text = "Device Connection Monitor";
        this.Size = new System.Drawing.Size(1000, 600);

        _dataManager = new DeviceDataManager();

        InitializeUI();
        LoadDataToUI();
        InitializeDeviceMonitoring();
    }

    private void InitializeDeviceMonitoring()
    {
        try
        {
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
            _insertWatcher = new ManagementEventWatcher(insertQuery);
            _insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            _insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
            _removeWatcher = new ManagementEventWatcher(removeQuery);
            _removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            _removeWatcher.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize device monitoring. Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
    {
        ProcessDeviceEvent(e, "Connected");
    }

    private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
    {
        ProcessDeviceEvent(e, "Disconnected");
    }

    private void ProcessDeviceEvent(EventArrivedEventArgs e, string eventType)
    {
        try
        {
            var targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string deviceId = targetInstance["DeviceID"]?.ToString() ?? "Unknown";
            string name = targetInstance["Name"]?.ToString() ?? "Unknown";
            string description = targetInstance["Description"]?.ToString() ?? "Unknown";

            var deviceEvent = new DeviceEvent
            {
                EventType = eventType,
                Timestamp = DateTime.Now,
                DeviceId = deviceId,
                Name = name,
                Description = description
            };

            _dataManager.AddEvent(deviceEvent);
            UpdateGrids();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing device event: {ex.Message}");
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        if (_insertWatcher != null)
        {
            _insertWatcher.Stop();
            _insertWatcher.Dispose();
        }
        if (_removeWatcher != null)
        {
            _removeWatcher.Stop();
            _removeWatcher.Dispose();
        }
    }

    private void InitializeUI()
    {
        _tabControl = new TabControl { Dock = DockStyle.Fill };

        // Summary Tab
        _tabSummary = new TabPage("Summary");
        _gridSummary = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _summaryBindingSource = new BindingSource();
        _gridSummary.DataSource = _summaryBindingSource;
        _tabSummary.Controls.Add(_gridSummary);

        // History Tab
        _tabHistory = new TabPage("History");
        _gridHistory = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        _historyBindingSource = new BindingSource();
        _gridHistory.DataSource = _historyBindingSource;
        _tabHistory.Controls.Add(_gridHistory);

        _tabControl.TabPages.Add(_tabSummary);
        _tabControl.TabPages.Add(_tabHistory);

        this.Controls.Add(_tabControl);
    }

    private void LoadDataToUI()
    {
        UpdateGrids();
    }

    private void UpdateGrids()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(UpdateGrids));
            return;
        }

        // Use binding list to auto-update UI if desired, or simple list reassignment
        _summaryBindingSource.DataSource = _dataManager.Data.Devices.Values.ToList();
        _historyBindingSource.DataSource = _dataManager.Data.Events.ToList();

        // Refresh grids
        _gridSummary.Refresh();
        _gridHistory.Refresh();
    }
}
