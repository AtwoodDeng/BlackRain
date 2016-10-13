To add the rain particle effect to your scene, simply drag the Rain prefab of your chosen size (Large, Medium or Small etc) to the Hierarchy. You may need to tweak its position on the Y axis so that the ripples and bubbles don't intersect with the ground. For the rain sound effect, simply drag the Rain_Audio prefab to the Hierarchy too.

To add thunder and lightning, again drag the Thunder_And_Lightning prefab to the Hierarchy. When selecting it, you'll notice in the Inspector that the script has a few settings; "Off Min" is the minimum wait time in seconds between each thunder/lighting event, while "Off Max" is the maximum wait time. A random wait time between these 2 values will be generated. You'll notice that the 4 thunder sound effects have been assigned, while LightningBolt is the lightning particle effect.

For optimum results, consider adding the "Fast Bloom" image effect to the camera (found under Image Effects -> Bloom and Glow -> Bloom). Some suggested bloom settings can be found in the BloomSettings.jpg image.

As well as manually placing the rain effect in your scene, an alternative option is to use the Prefab "Rain - LocalToPlayer". This provides a rain effect that's parented to the player, only displaying the particle effects near to the camera. This may be a better solution for larger, open environments. To use this effect, first set transformations of the First Person Controller (or whatever you are using) to 0. Then drag the "Rain - LocalToPlayer" onto the controller (not the camera node). Also, new to version 1.4 is a parented version of the rain that collides with the environment - "Rain - LocalToPlayer - Collides With Environment.prefab"

If you have any issues then please get in touch at bendurrant@rivermillstudios.com

Have fun!