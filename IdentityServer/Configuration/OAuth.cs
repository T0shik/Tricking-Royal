﻿namespace IdentityServer.Configuration
{
    public class OAuth
    {
        public Routing Routing { get; set; }
        public Resource Api { get; set; }
        public Resource Cdn { get; set; }
    }
}