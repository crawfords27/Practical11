using Microsoft.AspNetCore.Mvc;

namespace SMS.Web.Controllers;

public enum AlertType { success, danger, warning, info }

public class BaseController : Controller
{
    // Store Alert in TempData Storage thus only accessible in next Request
    public void Alert(string message, AlertType type = AlertType.info)
    {
        TempData["Alert.Message"] = message;
        TempData["Alert.Type"] = type.ToString();
    }

    // utility method to convert IFormFile to a base64 string
   
}
