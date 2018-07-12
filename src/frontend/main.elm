import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onInput)



main =
  Html.beginnerProgram { model = init, view = view, update = update }


-- MODEL

type alias Card = {
    cardId: Int
}

type alias Player = {
    name: String
    , marks: Int
    , hisTurn: Bool
    , openedCards: List Card
}

type alias Opponent = {
    name: String
    , marks: Int
}

type alias Game = {
    cards: List Card,
    player: Player,
    opponent: Opponent
}

initPlayer : Player
initPlayer =
    { name = ""
    , marks = 0
    , hisTurn = False
    , openedCards = []
    }

initOpponent : Opponent
initOpponent =
    { name = "Jules"
    , marks = 0
    }

init : Game
init = 
    { cards =
        [ Card 0
        , Card 1
        , Card 2
        , Card 3
        , Card 4
        , Card 5
        , Card 6
        , Card 7
        , Card 8
        , Card 9
        , Card 10
        , Card 11
        ]
    , player = initPlayer
    , opponent = initOpponent
    }

-- UPDATE

type Msg
    = IdCard Int

update : Msg -> Game -> Game
update msg game =
    case msg of 
        IdCard id ->
            selectCard id game

selectCard : Int -> Game -> Game
selectCard idCard game =
    if List.length game.player.openedCards == 2 then
        game
    else
        let op = 
                List.append game.player.openedCards (getCard idCard game)
            thePlayer = game.player
            nPlayer =
                {thePlayer | openedCards = op}
        in
            ({game | player = nPlayer})

isTheCard : Int -> Card -> Bool
isTheCard id card =
    if card.cardId == id then True else False

getCard : Int -> Game -> List Card
getCard idCard game =
    List.filter (isTheCard idCard) game.cards

-- VIEW


view : Game -> Html Msg
view game =
    let player =
            game.player
        opponent =
            game.opponent
    in
        (div []
        [ div[][text player.name]
        , div[][text ("Your Score: " ++ (toString player.marks))]
        , div[][text (opponent.name ++ " Score: " ++ (toString opponent.marks))]
        ])