﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendApi.Models
{
    public class AccessTokenResult
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("ext_expires_in")]
        public int ExtExpiresIn { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
