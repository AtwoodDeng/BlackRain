var offMin : float= 10; // Minimum wait time between each lightning/thunder
var offMax : float= 60; // Maximum wait time between each lightning/thunder

var ThunderAudioA : AudioSource;
var ThunderAudioB : AudioSource;
var ThunderAudioC : AudioSource;
var ThunderAudioD : AudioSource;
var LightningBolt : GameObject;

private var onMin : float= 0.25; // Minimum duration of lightning bolt flash
private var onMax : float= 2; // Maximum duration of lightning bolt flash
private var ThunderRND;
private var ThunderVol;
private var ThunderWait;

ThunderRND = 1;

  function Start() 
 {
 light();
 }
 
 function light()
 {
 
     
 
     while(true)
         {
         yield WaitForSeconds(Random.Range(offMin, offMax)); // Random delay before next lighning, between OffMin and Offmax

         LightningBolt.SetActive (true); // Show the lighning bolt particle effect 

         LightningBolt.transform.Rotate(0,(Random.Range(1, 360)),0); // Random direction of lighing bolt
         

         soundfx(); // Play thunder sound
         yield WaitForSeconds(Random.Range(onMin, onMax)); // Random duration of lightning flash
  
         LightningBolt.SetActive (false); // Hide the lighning bolt particle effect
         }
 }
 
 function soundfx()
 {

     // Choose a random thunder sound effect with random rolume

     ThunderRND = (Random.Range(1,5));
     ThunderVol = (Random.Range(0.2,1.0)); // Random thunder volume
     ThunderWait = ((9 - ((ThunderVol * 3)*3))-2); // The lower the thunder volulme the longer wait between lighting flash and thunder sound
     
   	while (ThunderRND == 1){
     yield WaitForSeconds(ThunderWait);
     ThunderAudioA.volume = ThunderVol;
     ThunderAudioA.Play();
     
     ThunderRND = 0;
     }     
 
   	while (ThunderRND == 2){
     yield WaitForSeconds(ThunderWait);
     ThunderAudioB.volume = ThunderVol;
     ThunderAudioB.Play();
     ThunderRND = 0;
     }
     
     while (ThunderRND == 3){
     yield WaitForSeconds(ThunderWait);
     ThunderAudioC.volume = ThunderVol;
     ThunderAudioC.Play();
     ThunderRND = 0;
     }


     while (ThunderRND == 4){
     yield WaitForSeconds(ThunderWait);
     ThunderAudioD.volume = ThunderVol;
     ThunderAudioD.Play();
     ThunderRND = 0;
     }

      
     
 }