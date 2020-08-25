#define BLYNK_PRINT Serial
#include <ESP8266WiFi.h>
#include <BlynkSimpleEsp8266.h>
char auth[] = "Tdvwhxra_-q2XxcdJfnb5MjkhL4k1hGB";
char ssid[] = "y";
char pass[] = "11111111";

BlynkTimer timer;

void myTimerEvent()
{
  // Blynk.virtualWrite(V5, millis() / 1000);
  if(Serial.available()>0){
    String value = Serial.readStringUntil(',');
    Blynk.virtualWrite(V0,value);
  }
}
void setup()
{
  Serial.begin(9600);
  Blynk.begin(auth, ssid, pass);
  timer.setInterval(100L, myTimerEvent);
}

void loop()
{
  Blynk.run();
  timer.run();
}
