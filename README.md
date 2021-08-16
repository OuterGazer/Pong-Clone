# Pong-Clone
A Pong clone developed with the XNA Framework as part of Rob Mile's "The Yellow Book" lab work.

My first game using a graphical interface.

To play it, you need to download the zip file, extract everything and run setup.exe.
Afterwards you need to download and install the XNA 4.0 libraries for the game to work (included in the repository).

The game needs 2 players. The player on the left controls the paddle with W and S, the player on the right controls the paddle with the Up and Down arrows.

I implemented the following features as extra from the original lab work:
- Players can enter their name.
- A background texture.
- The ball's speed increases slightly after each succesful paddle hit up to a max speed.
- Fixed a nasty bug in the original code where the ball would either get trapped behind the paddle or score a self goal if hit with the upper or lower edges of the paddle. Now this doesn't happen.
