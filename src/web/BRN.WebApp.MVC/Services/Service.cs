﻿using BRN.WebApp.MVC.Extensions;
using System.Net.Http;

namespace BRN.WebApp.MVC.Services
{
    public abstract class Service
    {
        protected bool HandleError(HttpResponseMessage response)
        {
            switch ((int)response.StatusCode)
            {
                case 401:
                case 403:
                case 404:
                case 500:
                    throw new CustomHttpRequestException(response.StatusCode);

                case 400:
                    return false;


            }

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
