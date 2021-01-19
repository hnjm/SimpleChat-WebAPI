using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Sentry;

namespace SimpleChat.Core.Helper
{
    public static class SystemIO
    {
        public static string ReadBodyAsString(this HttpRequest request)
        {
            try
            {
                var reader = new StreamReader(request.Body);
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                return reader.ReadToEnd();
            }
            catch (ArgumentNullException e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
            catch (ArgumentException e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
            catch (ObjectDisposedException e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
            catch (NotSupportedException e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
            catch (OutOfMemoryException e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
            catch (IOException e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return "";
            }
        }
    }
}
