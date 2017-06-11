// "Another easier inverted pendulum balancing robot"
// You need only half a day to make it, if you have some Materials.
// (This sketch is ver.2.0.d for a digital output gyroscope.)
// No timer library is used in this version.
// But stability of robot is more improved than earlier version.
// Copyright (C) 2014 ArduinoDeXXX All Rights Reserved.
//#include <MsTimer2.h> //01 (This line is omitted in ver.2.0 and the later.)
//int i = 0; //02 (This line is omitted in this version.)
byte countS = 0; //03
//long zeroOmegaI = 0; //04 (This line is omitted in this version.)
int recOmegaI[10]; //05
int omegaI = 0; //06
long thetaI = 0; //07
long sumPower = 0; //08
long sumSumP = 0; //09
const int kAngle = 50; //10
const int kOmega = 500; //11
const long kSpeed = 60; //12
const long kDistance = 20; //13
long powerScale; //14
int power; //15
long vE5 = 0; //16
long xE5 = 0; //17
#include <SPI.h> //DL1 (These 17 lines, DL1-DL17, are added in this version.)
int ry; //DL2
long R; //DL3
void L3GD20_write(byte reg, byte val) { //DL4
digitalWrite(10, LOW); //DL5
SPI.transfer(reg); //DL6
SPI.transfer(val); //DL7
digitalWrite(10, HIGH); //DL8
} //DL9
byte L3GD20_read(byte reg) { //DL10
byte ret = 0; //DL11
digitalWrite(10, LOW); //DL12
SPI.transfer(reg | 0x80); //DL13
ret = SPI.transfer(0); //DL14
digitalWrite(10, HIGH); //DL15
return ret; //DL16
} //DL17
void setup () { //18
Serial .begin(115200); //19
pinMode(4, OUTPUT); //20
pinMode(5, OUTPUT); //20-a
pinMode(6, OUTPUT); //21
pinMode(7, OUTPUT);
pinMode(8, OUTPUT);
pinMode(9, OUTPUT);
for ( int i = 0 ; i < 10 ; i++ ) { recOmegaI[i] = 0; } //25 ("int" is added instead of line 2 omitted.)
pinMode(10, OUTPUT); //DL18 (These 8 lines, DL18-DL25, are added in this version.)
digitalWrite(10, HIGH); //DL19
SPI.begin(); //DL20
SPI.setBitOrder(MSBFIRST); //DL21
SPI.setDataMode(SPI_MODE3); //DL22
SPI.setClockDivider(SPI_CLOCK_DIV2); //DL23
L3GD20_write(0x20, B11001111); //DL24
L3GD20_write(0x23, B00000000);// DL25
delay(300); //26
// training(); // (This line is omitted in this version.)
// MsTimer2::set(5, chkAndCtl); // (This line is omitted in ver.2.0 and the later.)
// MsTimer2::start(); // (This line is omitted in ver.2.0 and the later.)
} //30
void loop () { //31
chkAndCtl(); // NL1 (This line is added in ver.2.0 and the later.)
if ( power > 0 ) {// 32
analogWrite( 6, power );
digitalWrite( 4, HIGH );
digitalWrite( 5, LOW ); //35
analogWrite( 9, power );
digitalWrite( 7, HIGH );
digitalWrite( 8, LOW );
} else {
analogWrite( 6, - power ); //40
digitalWrite( 4, LOW );
digitalWrite( 5, HIGH );
analogWrite( 9, - power );
digitalWrite( 7, LOW );
digitalWrite( 8, HIGH ); //45
}
// delayMicroseconds(3600); // NL2 (This is omitted in this version.)
}
//void training(){ //48 (These 7 lines, 48-54, are omitted in this version.)
// delay (1000);
// for ( i = 0 ; i < 500 ; i++ ){ //50
// zeroOmegaI = zeroOmegaI + analogRead(A5);
// }
// zeroOmegaI = zeroOmegaI / i;
//} //54
void chkAndCtl() { //55
// omegaI = 0; // NL3 (These 6 lines, NL3-NL8, are omitted in this version.)
// for ( i = 0 ; i < 10 ; i++ ) { //NL4
// omegaI = omegaI + analogRead(A5) - zeroOmegaI; //NL5
// delayMicroseconds(10); //NL6
// } //NL7
// omegaI = omegaI / 10; //NL8
R = 0; //DL26 (These 7 lines, DL26-DL32, are added in this version.)
for ( int i = 0 ; i < 45 ; i++ ) {// DL27 ("int" is added instead of line 2 omitted.)
ry = ( (L3GD20_read(0x2B) << 8) | L3GD20_read(0x2A) ); //DL28
R = R + ry; //DL29
delayMicroseconds(90); //DL30
} //DL31
omegaI = R * 0.00875 / 45; //DL32
// omegaI = analogRead(A5) - zeroOmegaI; //56 (This line is omitted in ver.2.0 and the later.)
if ( abs( omegaI ) < 2 ) { omegaI = 0; } //57 (The lower bound is less than 2 in this version.)
recOmegaI[0] = omegaI;
thetaI = thetaI + omegaI;
countS = 0; //60
for ( int i = 0 ; i < 10 ; i++ ) {// ("int" is added instead of line 2 omitted.)
if ( abs( recOmegaI[i] ) < 4 ) { countS++; } //62 (The lower bound is less than 4 in this version.)
}
if ( countS > 9 ) {
thetaI = 0;// 65
vE5 = 0;
xE5 = 0;
sumPower = 0;
sumSumP = 0;
} //70
for ( int i = 9 ; i > 0 ; i-- ) { recOmegaI[ i ] = recOmegaI[ i-1 ]; }// ("int" is added instead of line 2 omitted.)
powerScale = ( kAngle * thetaI / 100 ) + ( kOmega * omegaI / 100 ) + ( kSpeed * vE5 / 1000 ) + ( kDistance * xE5 / 1000 ); //
power = max ( min ( 95 * powerScale / 100 , 255 ) , -255 );
sumPower = sumPower + power;
sumSumP = sumSumP + sumPower;// 75
// vE5 = ??? //76
// xE5 = ??? //77
} //78
// Copyright (C) 2014 ArduinoDeXXX All Rights Reserved. //79
