#define BLYNK_PRINT               Serial
#define fanPin                    D5
#define SDA_pin                   D2
#define SCL_pin                   D1
#define batasSuhu                 60
#include <ESP8266WiFi.h>
#include <BlynkSimpleEsp8266.h>
#include <ArduinoJson.h>
#include <LiquidCrystal_I2C.h>

char auth[] = "Tdvwhxra_-q2XxcdJfnb5MjkhL4k1hGB";
char ssid[] = "y";
char pass[] = "11111111";

BlynkTimer timer;
WidgetLCD blynkLcd(V4);
LiquidCrystal_I2C lcd(0x27,16,2);
BLYNK_WRITE(V2)
{
  Serial.write('b');
}
BLYNK_WRITE(V3)
{
  Serial.write('a');
}
void myTimerEvent()
{
  // Blynk.virtualWrite(V5, millis() / 1000);
  if(Serial.available()>0){
    DynamicJsonDocument output(1024);
    String data = Serial.readStringUntil('\n');
    deserializeJson(output, data);
    int load = output["load"];
    int temp = output["temp"];
    String time = output["time"];
    lcd.clear();
    lcd.setCursor(0,0);
    lcd.print("Suhu     : "+(String)temp+"C");
    lcd.setCursor(0,1);
    lcd.print("CPU Load : "+(String)load+"%");
    Blynk.virtualWrite(V0,load);
    Blynk.virtualWrite(V1,temp);
    blynkLcd.print(0,0,"LAST DATA");
    blynkLcd.print(0,1,time);
    if(temp>batasSuhu){
      Blynk.notify("Suhu mencapai "+(String)batasSuhu+" C");
      digitalWrite(fanPin,HIGH);
    }
    else digitalWrite(fanPin,LOW);

  }
}
void setup()
{
  Serial.begin(9600);
  pinMode(fanPin,OUTPUT);
  lcd.begin();
  lcd.setCursor(0,0);
  lcd.print("Menunggu data . .");
  Blynk.begin(auth, ssid, pass);
  timer.setInterval(100L, myTimerEvent);
}

void loop()
{
  Blynk.run();
  timer.run();
}