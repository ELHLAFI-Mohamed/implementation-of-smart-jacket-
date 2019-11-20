
#include <MPU6050_tockn.h>
#include <Wire.h>

MPU6050 mpu6050(Wire);

void setup() {
  Serial.begin(9600);
  Wire.begin();
  mpu6050.begin();
  mpu6050.calcGyroOffsets(true);
  long sumX,sumY,sumZ = 0;
  /*for (int i = 0; i < 20; i++){
    sumX += mpu6050.getAngleX();
    sumY += mpu6050.getAngleY();
    sumZ += mpu6050.getAngleZ();
    }
    sum*/
}

void loop() {
  mpu6050.update();
  //Serial.print("angleX : ");
  Serial.print(mpu6050.getAngleX());Serial.print(";");
  //Serial.print("\tangleY : ");
  Serial.print(mpu6050.getAngleY());Serial.print(";");
  //Serial.print("\tangleZ : ");
  Serial.print(mpu6050.getAngleZ());Serial.println(";");

  delay(100);
}
