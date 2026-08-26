using System;
using System.Collections.Generic;

namespace DesignPatternCatalog.Services.PatternImplementations;

public interface IPaymentProcessor
{
    string ProcessPayment(decimal amount, string currency, string customerId);
}

public class ModernStripeGateway : IPaymentProcessor
{
    public string ProcessPayment(decimal amount, string currency, string customerId)
    {
        string txnId = $"txn_9N82xA{DateTime.Now.Ticks % 100000}";
        return $"[NATIVE REST PAYMENT PROCESSOR (JSON)]\n" +
               $"------------------------------------------------------------\n" +
               $"• Interface Called:   IPaymentProcessor.ProcessPayment(₹{amount:F2}, \"{currency}\", \"{customerId}\")\n" +
               $"• Native Outgoing:    POST https://api.paymentgateway.com/v1/orders\n" +
               $"• Payload:            {{ \"amount\": {(long)(amount * 100)}, \"currency\": \"{currency}\", \"customer_id\": \"{customerId}\" }}\n" +
               $"• HTTP Response:     200 OK | Transaction ID: {txnId}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: Payment of ₹{amount:F2} approved natively via REST.";
    }
}

public class LegacyBankSoapSdk
{
    public string ExecuteXmlEnvelope(string xmlPayload)
    {
        return $"<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
               $"  <SOAP-ENV:Body>\n" +
               $"    <TransactionResponse>\n" +
               $"      <StatusCode>00</StatusCode>\n" +
               $"      <StatusMessage>APPROVED</StatusMessage>\n" +
               $"      <BankAuthCode>AUTH-{DateTime.Now.Ticks % 900000 + 100000}</BankAuthCode>\n" +
               $"    </TransactionResponse>\n" +
               $"  </SOAP-ENV:Body>\n" +
               $"</SOAP-ENV:Envelope>";
    }
}

public class LegacyBankAdapter : IPaymentProcessor
{
    private readonly LegacyBankSoapSdk _soapSdk;

    public LegacyBankAdapter(LegacyBankSoapSdk soapSdk)
    {
        _soapSdk = soapSdk;
    }

    public string ProcessPayment(decimal amount, string currency, string customerId)
    {
        string soapRequest = 
            $"<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
            $"  <SOAP-ENV:Body>\n" +
            $"    <ProcessTransaction>\n" +
            $"      <CustId>{customerId}</CustId>\n" +
            $"      <TxnAmount>{amount:F2}</TxnAmount>\n" +
            $"      <CurrencyCode>{currency}</CurrencyCode>\n" +
            $"    </ProcessTransaction>\n" +
            $"  </SOAP-ENV:Body>\n" +
            $"</SOAP-ENV:Envelope>";

        string soapResponse = _soapSdk.ExecuteXmlEnvelope(soapRequest);
        bool approved = soapResponse.Contains("<StatusCode>00</StatusCode>");

        return $"[ADAPTER PATTERN EXECUTION (LegacyBankAdapter)]\n" +
               $"------------------------------------------------------------\n" +
               $"• Step 1 (Client):    Calls standard IPaymentProcessor.ProcessPayment(₹{amount:F2}, \"{currency}\", \"{customerId}\")\n" +
               $"• Step 2 (Adapter):   LegacyBankAdapter intercepts and transforms JSON to SOAP XML payload\n" +
               $"• Step 3 (Adaptee):   Invoked LegacyBankSoapSdk.ExecuteXmlEnvelope()\n" +
               $"• Step 4 (Response):  {soapResponse}\n" +
               $"• Step 5 (Adapter):   Parsed <StatusCode>00</StatusCode> -> Success = {approved}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [SUCCESS] Legacy SOAP bank integrated with 0 changes to modern client code.";
    }
}

public interface IDevice
{
    bool IsEnabled { get; }
    void Enable();
    void Disable();
    int Volume { get; set; }
    string GetDeviceName();
    string ExecuteCustom(string command);
}

public class SonyBraviaTv : IDevice
{
    public bool IsEnabled { get; private set; } = true;
    public int Volume { get; set; } = 40;
    public string GetDeviceName() => "Sony Bravia 4K Smart TV";
    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
    public string ExecuteCustom(string command) => $"Sony TV tuned input to HDMI 1 (eARC). Volume: {Volume}%";
}

public class YamahaSoundbar : IDevice
{
    public bool IsEnabled { get; private set; } = true;
    public int Volume { get; set; } = 50;
    public string GetDeviceName() => "Yamaha Home Theater Receiver";
    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
    public string ExecuteCustom(string command) => $"Yamaha Audio processor active. Dolby Atmos 7.1 mode. Volume: {Volume}%";
}

public class DaikinAirConditioner : IDevice
{
    public bool IsEnabled { get; private set; } = true;
    public int Volume { get; set; } = 22;
    public string GetDeviceName() => "Daikin Climate Air Conditioner";
    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
    public string ExecuteCustom(string command) => $"Daikin Climate set to 22°C Eco Inverter mode.";
}

public class RemoteControl
{
    protected readonly IDevice _device;
    public RemoteControl(IDevice device) { _device = device; }

    public virtual string TogglePower()
    {
        if (_device.IsEnabled) { _device.Disable(); return $"Sent Power OFF signal to {_device.GetDeviceName()}."; }
        else { _device.Enable(); return $"Sent Power ON signal to {_device.GetDeviceName()}."; }
    }

    public virtual string VolumeUp()
    {
        _device.Volume = Math.Min(100, _device.Volume + 10);
        return $"Incremented volume on {_device.GetDeviceName()} to {_device.Volume}%.";
    }
}

public class AdvancedRemoteControl : RemoteControl
{
    public AdvancedRemoteControl(IDevice device) : base(device) { }

    public string Mute()
    {
        _device.Volume = 0;
        return $"Muted audio on {_device.GetDeviceName()}. Volume = 0%.";
    }

    public string VoiceCommand(string query)
    {
        return $"Processed Voice Command \"{query}\" -> {_device.ExecuteCustom(query)}";
    }
}

public interface IBeverage
{
    string GetDescription();
    decimal GetCost();
    List<string> GetLayers();
}

public class Espresso : IBeverage
{
    public string GetDescription() => "Signature Dark Espresso";
    public decimal GetCost() => 250.00m;
    public List<string> GetLayers() => new() { "Base Component: [Signature Dark Espresso] (₹250.00)" };
}

public class ColdBrew : IBeverage
{
    public string GetDescription() => "Cold Brew Reserve";
    public decimal GetCost() => 325.00m;
    public List<string> GetLayers() => new() { "Base Component: [Cold Brew Reserve] (₹325.00)" };
}

public class Americano : IBeverage
{
    public string GetDescription() => "Caffe Americano";
    public decimal GetCost() => 275.00m;
    public List<string> GetLayers() => new() { "Base Component: [Caffe Americano] (₹275.00)" };
}

public class BlondeRoast : IBeverage
{
    public string GetDescription() => "Vanilla Blonde Roast";
    public decimal GetCost() => 300.00m;
    public List<string> GetLayers() => new() { "Base Component: [Vanilla Blonde Roast] (₹300.00)" };
}

public abstract class BeverageDecorator : IBeverage
{
    protected readonly IBeverage _beverage;

    protected BeverageDecorator(IBeverage beverage)
    {
        _beverage = beverage;
    }

    public virtual string GetDescription() => _beverage.GetDescription();
    public virtual decimal GetCost() => _beverage.GetCost();
    public virtual List<string> GetLayers() => _beverage.GetLayers();
}

public class MilkDecorator : BeverageDecorator
{
    public MilkDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Steamed Oat Milk";
    public override decimal GetCost() => base.GetCost() + 70.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Wrapper Layer: MilkDecorator (+₹70.00)");
        return list;
    }
}

public class CaramelDecorator : BeverageDecorator
{
    public CaramelDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Salted Caramel Drizzle";
    public override decimal GetCost() => base.GetCost() + 80.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Wrapper Layer: CaramelDecorator (+₹80.00)");
        return list;
    }
}

public class VanillaDecorator : BeverageDecorator
{
    public VanillaDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Madagascar Vanilla";
    public override decimal GetCost() => base.GetCost() + 60.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Wrapper Layer: VanillaDecorator (+₹60.00)");
        return list;
    }
}

public class WhippedCreamDecorator : BeverageDecorator
{
    public WhippedCreamDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Whipped Cream Cloud";
    public override decimal GetCost() => base.GetCost() + 60.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Wrapper Layer: WhippedCreamDecorator (+₹60.00)");
        return list;
    }
}

public class ExtraShotDecorator : BeverageDecorator
{
    public ExtraShotDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Extra 2x Espresso Shot";
    public override decimal GetCost() => base.GetCost() + 120.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Wrapper Layer: ExtraShotDecorator (+₹120.00)");
        return list;
    }
}

public class LightsSubsystem
{
    public string Dim(int level) => $"SmartLightsController.Dim({level}%) -> Ambient movie mood active.";
    public string On() => $"SmartLightsController.SetBrightness(100%) -> Warm daylight restored.";
}

public class ProjectorSubsystem
{
    public string PowerOn() => $"4KLaserProjector.PowerOn() -> Input HDMI 1 (eARC 4K HDR).";
    public string PowerOff() => $"4KLaserProjector.PowerOff() -> Bulb cooling cycle active.";
}

public class AudioSubsystem
{
    public string SetProfile(string profile) => $"AudioReceiver.SetAudioProfile(\"{profile}\") -> Volume 65% calibrated.";
    public string PowerOff() => $"AudioReceiver.PowerOff() -> Audio muted.";
}

public class StreamingPlayerSubsystem
{
    public string Play(string title) => $"StreamingPlayer.PlayStream(\"{title}\") -> Bitrate 48 Mbps streaming.";
    public string Stop() => $"StreamingPlayer.Stop() -> Playback ended.";
}

public class HomeTheaterFacade
{
    private readonly LightsSubsystem _lights;
    private readonly ProjectorSubsystem _projector;
    private readonly AudioSubsystem _audio;
    private readonly StreamingPlayerSubsystem _player;

    public HomeTheaterFacade(LightsSubsystem lights, ProjectorSubsystem projector, AudioSubsystem audio, StreamingPlayerSubsystem player)
    {
        _lights = lights;
        _projector = projector;
        _audio = audio;
        _player = player;
    }

    public string WatchMovie(string title, string soundProfile)
    {
        return $"[FACADE EXECUTION: HomeTheaterFacade.WatchMovie(\"{title}\")]\n" +
               $"------------------------------------------------------------\n" +
               $"• Step 1: {_lights.Dim(15)}\n" +
               $"• Step 2: MotorizedProjectorScreen.Lower() -> 120-inch 16:9 screen locked.\n" +
               $"• Step 3: {_projector.PowerOn()}\n" +
               $"• Step 4: {_audio.SetProfile(soundProfile)}\n" +
               $"• Step 5: {_player.Play(title)}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [READY] Client triggered 5 complex subsystems with a single method call.";
    }

    public string EndMovie()
    {
        return $"[FACADE EXECUTION: HomeTheaterFacade.EndMovie()]\n" +
               $"------------------------------------------------------------\n" +
               $"1. {_player.Stop()}\n" +
               $"2. {_projector.PowerOff()}\n" +
               $"3. {_audio.PowerOff()}\n" +
               $"4. MotorizedProjectorScreen.Raise() -> Stored in ceiling bay.\n" +
               $"5. {_lights.On()}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: All 5 subsystems powered down in sequence.";
    }
}
