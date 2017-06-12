#include <SPI.h>
short X, Y, Z;
float x, y, z;
void L3GD20_write(byte reg, byte val){
 digitalWrite(10, LOW);
 SPI.transfer(reg);
 SPI.transfer(val);
 digitalWrite(10, HIGH);
}
byte L3GD20_read(byte reg){
 byte ret = 0;
 digitalWrite(10, LOW);
 SPI.transfer(reg | 0x80);
 ret = SPI.transfer(0);
 digitalWrite(10, HIGH);
 return ret;
}
void setup () {
 digitalWrite(10, HIGH);
 pinMode(10, OUTPUT);
 SPI.begin();
 SPI.setBitOrder(MSBFIRST);
 SPI.setDataMode(SPI_MODE3);
 SPI.setClockDivider(SPI_CLOCK_DIV2);
 Serial .begin(115200);
 while (!Serial ) {}
 Serial .println("ms,x,y,z");
 Serial .println(L3GD20_read(0x0f), HEX); // should show D4
 L3GD20_write(0x20, B11001111);
}
void loop () {
 X = L3GD20_read(0x29);
 x = X = (X << 8) | L3GD20_read(0x28);
 Y = L3GD20_read(0x2B);
 y = Y = (Y << 8) | L3GD20_read(0x2A);
 Z = L3GD20_read(0x2D);
 z = Z = (Z << 8) | L3GD20_read(0x2C);
 x *= 0.00875;
 y *= 0.00875;
 z *= 0.00875;
 Serial .print(millis()); Serial .print(",");
 Serial .print(x); Serial .print(",");
 Serial .print(y); Serial .print(",");
 Serial .println(z);
 delay(5);
}
