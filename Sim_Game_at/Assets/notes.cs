/*
//game idea

every day the buildings require a up keep cost

there is max tile distance an agent can go based on age

every day new estimate is given by taking into account all the buildings
how this works is that on the new day calc the actual taken out amount, take it out and then use that estimate is more like a thing to show how much was taken last time but could
have it on a fucntion called when placing a new buildign, could have it on a que tbh or whatever



the player places the council

the council wiil then spawn 4 people around
they have no home
but work for the council

the agents only work on the day at night they should go to the their homes, if no homes they take damage

they are tasked to get shit around
the shit around is distance sorted but instead of picking the first one its random betweent he first few
this means i also need a way to tell whattile everyone is going to

this is where the stamina might come in
depending on the resource level up, this is where the level up comes in, either faster or more resrouces 






the perc Game:

lets say we have in bank

123 wood
67 stone
20 sand
5 people



the next turn is going to take 

100 wood
20 stone
5 sand
only 2 places  against 5


clearly needs to build a house
(houses can hold children unitll adult age)  laso need to give  a timing thing for the growing up
needless to say they need to be female and male or mybe i svcrap that dont know





if the overall diff expected is 

-20 wood
20 stone
5 sand


would also techincally need to check how many people are present and if there are enough people to take a job

then clearly there is not enoguh wood being harvested to keep up the building, the player can turn off some buildings or one will be turned off at random
the people working there will not work and for everyday they dont work they take damage
after a certain amount of time we destroy the buidling, i guess both entities can have a healtth bar

there is going to be an issue where i will need to find a way to decide where to build the thing
Based on the middle tile being in a favourable place we could make passive income


goign back to the above thing the game should be able to turn off some of the things that take wood in that cycle and wait for next time there is enough to build more 
maybe keep a flag for what was missing on that cycle so next time there is enough 



trying to achieve the susrvavl curve wih the game deicding  is own things
this also means that we need to respawn stuff in once its taken out

can only build in range of the council or outposts, outpostss need to take resources if an outpost dies and the things around 
need a pop up about the bad stuff




need to have a whole thing


i want to add taxes, maybe a slider that cna tell hey you have 1.5x the mamoung needed per turn and you are makin this much do you want to sell some for gold and then buy other stuff
this could be the way to get new people

need more functionaltuy, could add a shop, agents by default take 5 food if they go to shop they take only 3 but the shop only has a certain amount a day and cna only attarcts 
housese that are close by

could there be a storage warehuose shits, if the idea of max stoager based ont he structure is put in


when the Ai is tired maybe set up shops on the day and need to ratio food and stuff

when it gets to night eachhouses will send someone to get some food from the shop
every buidling has its own storage, council act as a food 

build a path on every built building so the pathing doesnt fuckup

CUNT



actual thigns to do either way:

max the tiles or distance allowed to go based on age, need a way to notice when its coming back, we could have a tile type called entrance  ---  this has been set as the overall cost fo the path
this is so the ntrance and the path tiles dont need to be check also takes out some complexity, for example if the overall path is 20 tiles long but its on a apth tile its fine
but i might still add a force check on the destination if its an entrance tile

get the new buidlings to spawn with their classes

agents without houses need to patroll around 
this also goes with the time cycle, when at night they stop moving and recharge if no house they recharge half,


the arrow key shit should delete the thing and that it also delete the resources

setup the scriptable object



 */
