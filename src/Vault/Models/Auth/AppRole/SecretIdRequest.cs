﻿using System.Collections.Generic;
using Newtonsoft.Json;
using Vault.Util;

namespace Vault.Models.Auth.AppRole
{
    public class SecretIdRequest
    {
        [JsonProperty("metadata")]
        public Dictionary<string, string> MetaData { get; set; }

        [JsonProperty("cidr_list")]
        private string _cidrList
        {
            get { return StringUtil.ListToCsvString(CidrList); }
            set { CidrList = StringUtil.CsvStringToList(value); }
        }

        [JsonIgnore]
        public List<string> CidrList { get; set; }
    }
}
