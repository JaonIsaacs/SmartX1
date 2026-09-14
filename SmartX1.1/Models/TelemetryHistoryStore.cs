namespace SmartX1._1.Models;

/// <summary>
/// Stores historical telemetry readings per device using a jagged array
/// where each row represents one device's readings
/// and rows can grow independently without wasted memory 
/// </summary>
public class TelemetryHistoryStore
{
    private readonly Dictionary<string, int> _deviceRowIndex = new();
    private TelemetryRecord[][] _history = Array.Empty<TelemetryRecord[]>();
    private readonly int[] _rowCounts = Array.Empty<int>();
    private readonly List<int> _rowCountsList = new();
    private readonly object _lock = new();

    private const int InitialRowCapacity = 16;

    /// <summary>
    /// Appends a reading to the device's row, growing the jagged row as needed.
    /// </summary>
    public void Append(TelemetryRecord record)
    {
        lock (_lock)
        {
            if (!_deviceRowIndex.TryGetValue(record.DeviceId, out var rowIndex))
            {
                rowIndex = _deviceRowIndex.Count;
                _deviceRowIndex[record.DeviceId] = rowIndex;

                Array.Resize(ref _history, rowIndex + 1);
                _history[rowIndex] = new TelemetryRecord[InitialRowCapacity];
                _rowCountsList.Add(0);
            }

            var count = _rowCountsList[rowIndex];
            var row = _history[rowIndex];

            if (count == row.Length)
            {
                Array.Resize(ref row, row.Length * 2);
                _history[rowIndex] = row;
            }

            row[count] = record;
            _rowCountsList[rowIndex] = count + 1;
        }
    }

    /// <summary>
    /// Returns the full historical reading sequence for a single device.
    /// </summary>
    public IReadOnlyList<TelemetryRecord> GetDeviceHistory(string deviceId)
    {
        lock (_lock)
        {
            if (!_deviceRowIndex.TryGetValue(deviceId, out var rowIndex))
                return Array.Empty<TelemetryRecord>();

            var count = _rowCountsList[rowIndex];
            return _history[rowIndex].Take(count).ToList();
        }
    }

    /// <summary>
    /// Returns the entire jagged structure trimmed to actual row lengths
    /// (useful for exporting/inspecting the raw array-of-arrays layout).
    /// </summary>
    public TelemetryRecord[][] GetRawJaggedSnapshot()
    {
        lock (_lock)
        {
            var snapshot = new TelemetryRecord[_history.Length][];
            for (int i = 0; i < _history.Length; i++)
            {
                snapshot[i] = _history[i].Take(_rowCountsList[i]).ToArray();
            }
            return snapshot;
        }
    }
}