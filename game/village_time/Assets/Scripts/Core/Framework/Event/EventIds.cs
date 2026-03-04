using System.Collections.Generic;

public static class EventIds {
    private static List<string> _eventIds;

    static EventIds() {
        _eventIds = new List<string>();
    }
    public static class CoreEvents {
        private const string PART_ID = "Core.";

        /// <summary>
        /// args类型：string 工作发放完毕的线程池的Id
        /// </summary>
        public static readonly string WorkQueueDone = RegisterEventId(PART_ID, nameof(WorkQueueDone));
    }

    private static string RegisterEventId(string partId, string id) {
        string fullName = partId + id;
        _eventIds.Add(fullName);
        return fullName;
    }

    public static IReadOnlyList<string> GetEventIds() {
        return _eventIds;
    }
}

