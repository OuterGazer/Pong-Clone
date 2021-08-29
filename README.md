# Pong-Clone
A Pong clone developed with the XNA Framework as part of Rob Mile's "The Yellow Book" lab work.

Gameplay Preview

![Pong Clone](https://user-images.githubusercontent.com/71871620/131250632-b35cb44f-fab9-4437-ab7d-bab23d596622.gif)


My first game using a graphical interface.

To play it, you need to download the zip file, extract everything and run setup.exe.
Afterwards you need to download and install the XNA 4.0 libraries for the game to work (included in the repository).

The game needs 2 players. The player on the left controls the paddle with W and S, the player on the right controls the paddle with the Up and Down arrows. To serve the ball at the beginning, players need to press space. The game can be paused by pressin P and quit by pressing Esc.

I implemented the following features as extra from the original lab work:
- Players can enter their name.
- A background texture.
- Now the ball starts at a paddle instead of moving directly on the field.
- Extra controls like pressing P to pause/unpause, Esc for quitting and space for serving the ball after scoring.
- The paddles' and ball's speeds increase slightly after each succesful paddle hit up to a max speed.
- Fixed a nasty bug in the original code where the ball would either get trapped behind the paddle or score a self goal if hit with the upper or lower edges of the paddle. Now this doesn't happen.
