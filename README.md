# Small Basic Challenges

This projet is some research for un concept of Small Basic Challenge between two Players.

## Concept

The purpose is to provide some challenges (or games) where two players create a Small Basic program which need to win the challenge or his opponent.

The communication system need to be very simple because Small Basic don't have High Level communication features, and need work without extensions.

This is not a real time communication system, only game that can be work turn-by-turn should supported.

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
	- If it waiting oppoenent
		- Wait and restart Game Phase
	- If game finished go to End Phase
	- Wait and restart Game Phase
- End Phase
	- Ask game result or history
	- Display result

## Small Basic Communication

For the communication, Small Basic will use only the `Network.GetWebPageContents()`. So SB send information with the url query, and the server returns a string value in Small Basic Array format.
  


