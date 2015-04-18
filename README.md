# Small Basic Challenges

This projet is some research for a concept of Small Basic Challenge between two Players.

## Concept

The purpose is to provide some challenges (or games) where two players create a Small Basic program to win the challenge or his opponent.

The communication system need to be very simple because Small Basic don't have High Level communication features, and need work without extensions.

This is not a real time communication system, only game that can be work turn-by-turn are more adapted with this system.

The process :

- Connection Phase
	- Ask a connection to an opponent
	- If Success save the connection info and go to Game Phase
	- If too tries aborted
	- Wait and restart the Connection Phase
- Game Phase
	- Ask game status from connection info
	- If it the player turn
		- Calculate his decision
		- Send to server is decision and receive status
		- If failed retry
		- If game finished go to End Phase
	- If it waiting opponent
		- Wait and restart Game Phase
	- If game finished go to End Phase
	- Wait and restart Game Phase
- End Phase
	- Ask game result or history
	- Display result

## Small Basic Communication

For the communication, Small Basic will use only the `Network.GetWebPageContents()`. So SB send information with the url query, and the server returns a string value in Small Basic Array format.

This array always contains an index **status** that indicates the response status, standard status are:
- **failed** : an error raised while processing the action
- **waiting** : the player is in the waiting list
- **connecting** : an opponent is found so the server try to connect the players
- **connected** : the players are connected, so the game start

When connected, the results have a **token** containing a value to identifying the player in the game session. This token need to be tramsitted for each other actions.

When playing, the status depends on the game. Each game provide is result, and states. Look the game rules to known how handling his states and results.

When the game is finished, we are in this situations :
- **aborted** : One of the player stop the game or break the communication, so the game his stopped. Some games handle the aborted situation by making the opponent has winner.
- **winner** : the player asking the action wins the game
- **looser** : the player asking the action loose the game

  
### Error handling

When we receive a **failed** status we have a **message** index containing the error message.

### Connection

The connection phase use the url `http://[GameServerUrl]/connect?` and require two parameters :
- **player** : Contains the player name display to the opponent
- **game** : Name of game we want to play

For example :  `http://[GameServerUrl]/connect?game=test&player=My Name`.

While we don't reach the **connected** status, then we are in the connection phase, and we should call the 'connect' action.

When we are connected, we need to save the **token** result indexed value to the rest of the game.

The informations in the result are :
- **token** : contains the game session identifier
- **playernum** : Number of the player in the session (1 or 2)
- **opponent** : the display name of the opponent
- **status** : the status


### Playing

While playing, we need to call the the url action `http://[GameServerUrl]/status?` with the **token** parameter to known the game status. For example :

`http://[GameServerUrl]/status?token=XXYYZZAA`.

This action returns an array with a **status** indexed value, indicating the game status. If we have **aborted**, **winner** or **looser** then the game is finished. Else we are in a game status depending of the rules of the game.

When it's your turn to play, you should call the url action `http://[GameServerUrl]/play?` with the parameters :
- **token** : containing the token session
- **command** : A value depending of the game
 
This play command permit you to tell to the game your action in the game. The results is the same as the 'status' action.

Reports to the game rules to known how handling the game playing phase, the results and the commands.

