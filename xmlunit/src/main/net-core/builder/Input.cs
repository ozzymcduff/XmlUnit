/*
   Licensed to the XMLUnit developers under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   This file is licensed to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System.Xml;

namespace net.sf.xmlunit.builder {
    public static class Input {
        internal class Source : ISource {
            private readonly XmlReader reader;
            internal Source(XmlReader r) {
                reader = r;
            }
            public XmlReader Reader {
                get {
                    return reader;
                }
            }
        }

        public interface IBuilder {
            ISource Build();
        }

        internal class DOMBuilder : IBuilder {
            private readonly ISource source;
            internal DOMBuilder(XmlDocument d) {
                source = new Source(new XmlNodeReader(d));
            }
            public ISource Build() {
                return source;
            }
        }

        public static IBuilder FromDocument(XmlDocument d) {
            return new DOMBuilder(d);
        }
    }
}
