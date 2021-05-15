﻿using IdentityServer4.Models;

namespace Identity.API.Models
{
    public record ErrorViewModel
    {
        public ErrorMessage Error { get; set; }

        public ErrorViewModel()
        {

        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }
    }
}
