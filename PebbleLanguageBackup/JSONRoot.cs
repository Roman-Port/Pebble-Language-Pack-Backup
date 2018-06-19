using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PebbleLanguageBackup
{
    [DataContract]
    class JSONRoot
    {
        [DataMember]
        public LanguagePack[] languages;
    }

    [DataContract]
    class LanguagePack
    {
        [DataMember]
        public string ISOLocal;
        [DataMember]
        public string file;
        [DataMember]
        public string firmware;
        [DataMember]
        public string hardware;
        [DataMember]
        public string localName;
        [DataMember]
        public LP_Mobile mobile;
        [DataMember]
        public string name;
        [DataMember]
        public float version;
        [DataMember]
        public string id;
    }

    [DataContract]
    class LP_Mobile
    {
        [DataMember]
        public string name;
        [DataMember]
        public string version;
    }
}
