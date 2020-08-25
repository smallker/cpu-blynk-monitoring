// #define BLYNK_PRINT Serial
// #define BLYNK_DEBUG
#include <Arduino.h>
#include <ESP8266WiFi.h>
#include <BlynkSimpleEsp8266.h>
#include <ArduinoJson.h>
char auth[] = "Tdvwhxra_-q2XxcdJfnb5MjkhL4k1hGB";
char ssid[] = "y";
char pass[] = "11111111";

BlynkTimer timer;
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
    String load = output["load"];
    String temp = output["temp"];
    Blynk.virtualWrite(V0,load);
    Blynk.virtualWrite(V1,temp);
  }
}
void setup()
{
  Serial.begin(9600);
  Blynk.begin(auth, ssid, pass);
  timer.setInterval(100L, myTimerEvent);
  pinMode(D2, INPUT_PULLUP);
}

void loop()
{
  Blynk.run();
  timer.run();
}
