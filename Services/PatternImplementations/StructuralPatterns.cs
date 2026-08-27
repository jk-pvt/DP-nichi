using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatternCatalog.Services.PatternImplementations;

public interface IPaymentProcessor
{
    string GatewayName { get; }
    string ProcessPayment(decimal amount, string currency, string customerId, HashSet<string> securityOptions);
}

public class ModernStripeGateway : IPaymentProcessor
{
    public string GatewayName => "Modern UPI & REST API Gateway (Native JSON)";

    public string ProcessPayment(decimal amount, string currency, string customerId, HashSet<string> securityOptions)
    {
        string txnId = $"txn_REST_{DateTime.Now.Ticks % 900000 + 100000}";
        double feePercent = currency == "INR" ? 0.0 : currency == "USD" ? 2.5 : currency == "EUR" ? 2.8 : 3.0;
        decimal forexFee = amount * (decimal)(feePercent / 100.0);
        decimal finalAmount = amount + forexFee;

        bool is2fa = securityOptions.Any(o => o.Contains("2FA"));
        bool isFraud = securityOptions.Any(o => o.Contains("Fraud"));
        bool isExpress = securityOptions.Any(o => o.Contains("Express"));
        bool isTax = securityOptions.Any(o => o.Contains("GST"));

        if (isExpress) finalAmount += 50m;
        decimal taxAmount = isTax ? finalAmount * 0.18m : 0m;
        decimal totalCharged = finalAmount + taxAmount;

        var payload =
            "{\n" +
            $"  \"merchant_id\": \"MERCH_INDIA_9921\",\n" +
            $"  \"customer_account\": \"{customerId}\",\n" +
            $"  \"base_amount\": {amount:F2},\n" +
            $"  \"currency\": \"{currency}\",\n" +
            $"  \"forex_fee\": {forexFee:F2},\n" +
            $"  \"tax_gst_18\": {taxAmount:F2},\n" +
            $"  \"total_settlement\": {totalCharged:F2},\n" +
            $"  \"security_flags\": {{ \"2fa_verified\": {is2fa.ToString().ToLower()}, \"fraud_risk_score\": \"{(isFraud ? "0.02 (VERY LOW)" : "UNCHECKED")}\" }}\n" +
            "}";

        return $"[NATIVE REST JSON GATEWAY EXECUTION]\n" +
               $"------------------------------------------------------------\n" +
               $"• Active Adapter:    {GatewayName}\n" +
               $"• Customer ID:       {customerId}\n" +
               $"• Base Amount:       ₹{amount:N2} ({currency})\n" +
               $"• Total Charged:     ₹{totalCharged:N2} (Includes taxes/fees)\n" +
               $"• HTTP Request:      POST https://api.paymentgateway.com/v2/charge\n" +
               $"• Protocol Payload (Native JSON):\n{payload}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [200 OK] Transaction ID: {txnId} processed natively via JSON.";
    }
}

public class LegacyBankSoapSdk
{
    public string ExecuteXmlEnvelope(string xmlPayload)
    {
        string authCode = $"SWIFT-AUTH-{DateTime.Now.Ticks % 900000 + 100000}";
        return "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
               "  <SOAP-ENV:Body>\n" +
               "    <TransactionResponse>\n" +
               "      <StatusCode>00</StatusCode>\n" +
               "      <StatusDescription>TRANSACTION_SETTLED_SUCCESSFULLY</StatusDescription>\n" +
               $"      <BankAuthCode>{authCode}</BankAuthCode>\n" +
               "      <ClearingNetwork>SWIFT_CORE_RTGS</ClearingNetwork>\n" +
               "    </TransactionResponse>\n" +
               "  </SOAP-ENV:Body>\n" +
               "</SOAP-ENV:Envelope>";
    }
}

public class LegacyBankAdapter : IPaymentProcessor
{
    private readonly LegacyBankSoapSdk _soapSdk;

    public LegacyBankAdapter(LegacyBankSoapSdk soapSdk)
    {
        _soapSdk = soapSdk;
    }

    public string GatewayName => "Legacy Banking SOAP Adapter (XML Translator)";

    public string ProcessPayment(decimal amount, string currency, string customerId, HashSet<string> securityOptions)
    {
        double feePercent = currency == "INR" ? 0.0 : currency == "USD" ? 2.5 : currency == "EUR" ? 2.8 : 3.0;
        decimal forexFee = amount * (decimal)(feePercent / 100.0);
        decimal finalAmount = amount + forexFee;

        bool isExpress = securityOptions.Any(o => o.Contains("Express"));
        bool isTax = securityOptions.Any(o => o.Contains("GST"));
        bool is2fa = securityOptions.Any(o => o.Contains("2FA"));

        if (isExpress) finalAmount += 50m;
        decimal taxAmount = isTax ? finalAmount * 0.18m : 0m;
        decimal totalCharged = finalAmount + taxAmount;

        string soapRequest =
            "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n" +
            "  <SOAP-ENV:Header>\n" +
            $"    <SecurityToken>AUTH_2FA_{(is2fa ? "VERIFIED_PASS" : "BYPASSED")}</SecurityToken>\n" +
            "  </SOAP-ENV:Header>\n" +
            "  <SOAP-ENV:Body>\n" +
            "    <ProcessPaymentTransaction>\n" +
            $"      <AccountIdentifier>{customerId}</AccountIdentifier>\n" +
            $"      <PrincipalAmount>{amount:F2}</PrincipalAmount>\n" +
            $"      <TotalSettlementAmount>{totalCharged:F2}</TotalSettlementAmount>\n" +
            $"      <CurrencyCode>{currency}</CurrencyCode>\n" +
            "    </ProcessPaymentTransaction>\n" +
            "  </SOAP-ENV:Body>\n" +
            "</SOAP-ENV:Envelope>";

        string soapResponse = _soapSdk.ExecuteXmlEnvelope(soapRequest);

        return $"[ADAPTER PATTERN EXECUTION (LegacyBankAdapter)]\n" +
               $"------------------------------------------------------------\n" +
               $"• Step 1 (Client):   Invoked standard IPaymentProcessor.ProcessPayment(₹{amount:N2})\n" +
               $"• Step 2 (Adapter):  Translated client request into legacy SOAP XML envelope\n" +
               $"• Step 3 (Outgoing Payload to Legacy Bank):\n{soapRequest}\n\n" +
               $"• Step 4 (Bank Response Envelope):\n{soapResponse}\n" +
               $"• Step 5 (Adapter):  Extracted <StatusCode>00</StatusCode> -> Success = True\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [ADAPTER SUCCESS] Total of ₹{totalCharged:N2} processed through legacy core banking.";
    }
}

public interface IDevice
{
    string DeviceName { get; }
    bool IsPowered { get; }
    int VolumeOrLevel { get; set; }
    string PowerToggle();
    string SetLevel(int level);
    string ApplyMode(string mode);
    string GetStatus();
}

public class SonyTvDevice : IDevice
{
    public string DeviceName => "Sony Bravia 4K OLED Smart TV";
    public bool IsPowered { get; private set; } = true;
    public int VolumeOrLevel { get; set; } = 45;

    public string PowerToggle() { IsPowered = !IsPowered; return $"Sony TV Power is now {(IsPowered ? "ON (HDMI 1 Active)" : "STANDBY")}."; }
    public string SetLevel(int level) { VolumeOrLevel = Math.Clamp(level, 0, 100); return $"Sony TV Volume calibrated to {VolumeOrLevel}%."; }
    public string ApplyMode(string mode) => $"Sony TV Picture Engine switched to [{mode}] mode with HDR10+ calibrated.";
    public string GetStatus() => $"Device: {DeviceName} | Power: {(IsPowered ? "ON" : "OFF")} | Volume: {VolumeOrLevel}%";
}

public class YamahaAudioDevice : IDevice
{
    public string DeviceName => "Yamaha Dolby Atmos 7.2 Home Theater Receiver";
    public bool IsPowered { get; private set; } = true;
    public int VolumeOrLevel { get; set; } = 60;

    public string PowerToggle() { IsPowered = !IsPowered; return $"Yamaha Receiver Power is now {(IsPowered ? "ON (7.2 Speakers Active)" : "OFF")}."; }
    public string SetLevel(int level) { VolumeOrLevel = Math.Clamp(level, 0, 100); return $"Yamaha Audio Master Gain set to {VolumeOrLevel}% ({VolumeOrLevel - 80} dB)."; }
    public string ApplyMode(string mode) => $"Yamaha DSP Audio Processor engaged [{mode}] spatial acoustic field.";
    public string GetStatus() => $"Device: {DeviceName} | Power: {(IsPowered ? "ON" : "OFF")} | Volume: {VolumeOrLevel}%";
}

public class DaikinAcDevice : IDevice
{
    public string DeviceName => "Daikin Inverter Climate Air Conditioner";
    public bool IsPowered { get; private set; } = true;
    public int VolumeOrLevel { get; set; } = 22;

    public string PowerToggle() { IsPowered = !IsPowered; return $"Daikin AC is now {(IsPowered ? "COOLING ACTIVE" : "OFF")}."; }
    public string SetLevel(int level) { VolumeOrLevel = Math.Clamp(level, 16, 30); return $"Daikin Thermostat temperature set to {VolumeOrLevel}°C."; }
    public string ApplyMode(string mode) => $"Daikin Climate Inverter set to [{mode}] mode.";
    public string GetStatus() => $"Device: {DeviceName} | Power: {(IsPowered ? "ON" : "OFF")} | Temp: {VolumeOrLevel}°C";
}

public class PhilipsHueDevice : IDevice
{
    public string DeviceName => "Philips Hue Smart Ambient Light Strip";
    public bool IsPowered { get; private set; } = true;
    public int VolumeOrLevel { get; set; } = 75;

    public string PowerToggle() { IsPowered = !IsPowered; return $"Hue Ambient Lights are now {(IsPowered ? "ILLUMINATED" : "OFF")}."; }
    public string SetLevel(int level) { VolumeOrLevel = Math.Clamp(level, 0, 100); return $"Hue Light Brightness dimmed to {VolumeOrLevel}%."; }
    public string ApplyMode(string mode) => $"Hue Color Palette changed to [{mode}] scene.";
    public string GetStatus() => $"Device: {DeviceName} | Power: {(IsPowered ? "ON" : "OFF")} | Brightness: {VolumeOrLevel}%";
}

public class RemoteControl
{
    protected readonly IDevice _device;
    public RemoteControl(IDevice device) { _device = device; }

    public virtual string RemoteType => "Basic Physical Remote";

    public virtual string Power() => _device.PowerToggle();
    public virtual string SetLevel(int level) => _device.SetLevel(level);
    public virtual string SendSpecial(string cmd) => _device.ApplyMode(cmd);
    public virtual string CheckStatus() => _device.GetStatus();
}

public class VoiceRemoteControl : RemoteControl
{
    public VoiceRemoteControl(IDevice device) : base(device) { }
    public override string RemoteType => "AI Voice Assistant Remote (Siri / Alexa)";

    public override string SendSpecial(string cmd) =>
        $"[Voice Command \"Hey Assistant, set {cmd}\"] -> {_device.ApplyMode(cmd)}";
}

public class TouchAppRemoteControl : RemoteControl
{
    public TouchAppRemoteControl(IDevice device) : base(device) { }
    public override string RemoteType => "Mobile App Remote (iOS & Android Touch)";

    public override string SendSpecial(string cmd) =>
        $"[Touch Screen Widget Slider Triggered \"{cmd}\"] -> {_device.ApplyMode(cmd)}";
}

public interface IBeverage
{
    string GetDescription();
    decimal GetCost();
    List<string> GetLayers();
}

public class Espresso : IBeverage
{
    public string GetDescription() => "Single-Origin Dark Espresso";
    public decimal GetCost() => 220.00m;
    public List<string> GetLayers() => new() { "Base: [Single-Origin Dark Espresso] (₹220.00)" };
}

public class ColdBrew : IBeverage
{
    public string GetDescription() => "Nitro Cold Brew Reserve";
    public decimal GetCost() => 310.00m;
    public List<string> GetLayers() => new() { "Base: [Nitro Cold Brew Reserve] (₹310.00)" };
}

public class Americano : IBeverage
{
    public string GetDescription() => "Caffe Americano Roast";
    public decimal GetCost() => 260.00m;
    public List<string> GetLayers() => new() { "Base: [Caffe Americano Roast] (₹260.00)" };
}

public class BlondeRoast : IBeverage
{
    public string GetDescription() => "Velvet Blonde Roast";
    public decimal GetCost() => 290.00m;
    public List<string> GetLayers() => new() { "Base: [Velvet Blonde Roast] (₹290.00)" };
}

public class MatchaLatte : IBeverage
{
    public string GetDescription() => "Matcha Green Tea Latte";
    public decimal GetCost() => 340.00m;
    public List<string> GetLayers() => new() { "Base: [Matcha Green Tea Latte] (₹340.00)" };
}

public abstract class BeverageDecorator : IBeverage
{
    protected readonly IBeverage _beverage;
    public BeverageDecorator(IBeverage beverage) { _beverage = beverage; }

    public virtual string GetDescription() => _beverage.GetDescription();
    public virtual decimal GetCost() => _beverage.GetCost();
    public virtual List<string> GetLayers() => _beverage.GetLayers();
}

public class SizeDecorator : BeverageDecorator
{
    private readonly string _sizeName;
    private readonly decimal _extraCost;

    public SizeDecorator(IBeverage beverage, string sizeName, decimal extraCost) : base(beverage)
    {
        _sizeName = sizeName;
        _extraCost = extraCost;
    }

    public override string GetDescription() => $"{_sizeName} {base.GetDescription()}";
    public override decimal GetCost() => base.GetCost() + _extraCost;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add($"Size Layer: [{_sizeName}] (+₹{_extraCost:F2})");
        return list;
    }
}

public class MilkDecorator : BeverageDecorator
{
    public MilkDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Steamed Oat Milk";
    public override decimal GetCost() => base.GetCost() + 70.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Topping: MilkDecorator (+₹70.00)");
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
        list.Add("Topping: CaramelDecorator (+₹80.00)");
        return list;
    }
}

public class VanillaDecorator : BeverageDecorator
{
    public VanillaDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Madagascar Vanilla Syrup";
    public override decimal GetCost() => base.GetCost() + 60.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Topping: VanillaDecorator (+₹60.00)");
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
        list.Add("Topping: WhippedCreamDecorator (+₹60.00)");
        return list;
    }
}

public class ExtraShotDecorator : BeverageDecorator
{
    public ExtraShotDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, 2x Extra Espresso Shot";
    public override decimal GetCost() => base.GetCost() + 120.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Topping: ExtraShotDecorator (+₹120.00)");
        return list;
    }
}

public class HazelnutDecorator : BeverageDecorator
{
    public HazelnutDecorator(IBeverage beverage) : base(beverage) { }
    public override string GetDescription() => $"{base.GetDescription()}, Roasted Hazelnut Crunch";
    public override decimal GetCost() => base.GetCost() + 45.00m;
    public override List<string> GetLayers()
    {
        var list = base.GetLayers();
        list.Add("Topping: HazelnutDecorator (+₹45.00)");
        return list;
    }
}

public class LightingSubsystem
{
    public string Dim(string mood) => $"LightingSubsystem: Lights dimmed to match [{mood}].";
    public string Restore() => "LightingSubsystem: Warm 100% room lighting restored.";
}

public class AudioSubsystem
{
    public string Calibrate(string profile, int volumeDb) =>
        $"AudioSubsystem: DSP Acoustic Profile [{profile}] active at {volumeDb} dB.";
    public string PowerOff() => "AudioSubsystem: Amplifier muted and powered off.";
}

public class ProjectorSubsystem
{
    public string Start(string title) => $"LaserProjector: 4K HDR Laser active. Input stream: \"{title}\".";
    public string Stop() => "LaserProjector: Laser turned off; fan cooling cycle started.";
}

public class MotorizedSubsystem
{
    public string Deploy(HashSet<string> toggles)
    {
        var actions = new List<string>();
        if (toggles.Any(t => t.Contains("Projector Screen"))) actions.Add("Lowered 130-inch Motorized Screen");
        if (toggles.Any(t => t.Contains("Blinds"))) actions.Add("Closed Motorized Blackout Blinds");
        if (toggles.Any(t => t.Contains("Popcorn"))) actions.Add("Pre-heated Smart Popcorn Machine");
        if (toggles.Any(t => t.Contains("Haptic"))) actions.Add("Engaged Subwoofer Haptic Bass Shakers");

        return actions.Count > 0
            ? "MotorizedSubsystem: " + string.Join(", ", actions) + "."
            : "MotorizedSubsystem: Standard equipment initialized.";
    }

    public string Retract() => "MotorizedSubsystem: Screen raised into ceiling bay; blinds reopened.";
}

public class HomeTheaterFacade
{
    private readonly LightingSubsystem _lights = new();
    private readonly AudioSubsystem _audio = new();
    private readonly ProjectorSubsystem _projector = new();
    private readonly MotorizedSubsystem _motorized = new();

    public string WatchMovie(string title, int volumeDb, string audioProfile, string lightingMood, HashSet<string> peripheralToggles)
    {
        return $"[FACADE EXECUTION: HomeTheaterFacade.WatchMovie()]\n" +
               $"------------------------------------------------------------\n" +
               $"• 1. {_lights.Dim(lightingMood)}\n" +
               $"• 2. {_motorized.Deploy(peripheralToggles)}\n" +
               $"• 3. {_audio.Calibrate(audioProfile, volumeDb)}\n" +
               $"• 4. {_projector.Start(title)}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: [READY] All 4 subsystems orchestrated dynamically with a single facade call.";
    }

    public string EndMovie()
    {
        return $"[FACADE EXECUTION: HomeTheaterFacade.EndMovie()]\n" +
               $"------------------------------------------------------------\n" +
               $"• 1. {_projector.Stop()}\n" +
               $"• 2. {_audio.PowerOff()}\n" +
               $"• 3. {_motorized.Retract()}\n" +
               $"• 4. {_lights.Restore()}\n" +
               $"------------------------------------------------------------\n" +
               $"STATUS: All equipment cleanly powered down in reverse sequence.";
    }
}
