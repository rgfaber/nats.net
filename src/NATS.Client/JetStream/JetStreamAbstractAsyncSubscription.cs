﻿// Copyright 2023 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace NATS.Client.JetStream
{
    public abstract class JetStreamAbstractAsyncSubscription : AsyncSubscription
    {
        internal readonly MessageManager MessageManager;
        internal string _consumer;

        // properties of IJetStreamSubscription, needed in subclass implementation
        public JetStream Context { get; }
        public string Stream { get; }
        public string Consumer => _consumer;
        public string DeliverSubject { get; }

        internal JetStreamAbstractAsyncSubscription(Connection conn, string subject, string queue,
            JetStream js, string stream, string consumer, string deliver, MessageManager messageManager)
            : base(conn, subject, queue)
        {
            Context = js;
            Stream = stream;
            _consumer = consumer; // might be null, JetStream will call UpdateConsumer later
            DeliverSubject = deliver;
            MessageManager = messageManager;
            MessageManager.Startup((IJetStreamSubscription)this);
        }

        internal virtual void UpdateConsumer(string consumer)
        {
            _consumer = consumer;
        }

        public ConsumerInfo GetConsumerInformation() => Context.LookupConsumerInfo(Stream, Consumer);

        public override void Unsubscribe()
        {
            MessageManager.Shutdown();
            base.Unsubscribe();
        }

        internal override void close()
        {
            MessageManager.Shutdown();
            base.close();
        }
    }
}
