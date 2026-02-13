public class QueueCfg {
    public readonly string id;
    public ushort maxThreads;
    public ushort maxJoinMs;
    // 需要一些性能监控参数，用来给弹性容量提供支持

    public QueueCfg(string id, ushort maxThreads = 1, ushort maxJoinMs = 100) {
        this.id = id;
        this.maxThreads = maxThreads;
        this.maxJoinMs = maxJoinMs;
    }

    public class Builder {
        private string _id;
        private ushort _maxThreads = 1;
        private ushort _maxJoinMs = 100;
        public Builder(string id) {
            this._id = id;
        }
        public QueueCfg Build() {
            return new QueueCfg(
                this._id,
                this._maxThreads,
                this._maxJoinMs
            );
        }
        public Builder Id(string id) {
            this._id = id;
            return this;
        }
        public Builder MaxThreads(ushort maxThreads) {
            this._maxThreads = maxThreads;
            return this;
        }
        public Builder MaxJoinMs(ushort maxJoinMs) {
            this._maxJoinMs = maxJoinMs;
            return this;
        }
    }

    public static Builder GetBuilder(string id) {
        return new Builder(id);
    }
}