import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick)
import Array
import Http
import Json.Decode as JD
import Json.Encode as JE


main =
  Html.beginnerProgram { model = init, view = view, update = update }

api =
    "https://61d9262f.ngrok.io/"

mu : String -> String
mu url =
    (api ++ url)

createPlayer : Http.Request User
createPlayer =
    Http.get (mu "player") userDecoder

userDecoder : JD.Decoder User
userDecoder =
    JD.map2 User
        (JD.field "id" JD.int)
        (JD.field "name" JD.string)

-- MODEL
type alias User = {
    id: Int
    ,name: String
}

type alias Card = {
    cardId: String,
    isDiscover: Bool
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
    { name = "Opponent"
    , marks = 0
    }

initCard: String -> Card
initCard cardId =
    { cardId = cardId
    , isDiscover = False
    }

init : Game
init = 
    { cards =
        [ initCard "0"
        , initCard "1"
        , initCard "2"
        , initCard "3"
        , initCard "4"
        , initCard "5"
        , initCard "2a"
        , initCard "4a"
        , initCard "0a"
        , initCard "3a"
        , initCard "1a"
        , initCard "5a"
        ]
    , player = initPlayer
    , opponent = initOpponent
    }

-- UPDATE
type Msg
    = CardId String

update : Msg -> Game -> Game
update msg game =
    case msg of
        CardId cardId ->
            (selectCard cardId game)

selectCard : String -> Game -> Game
selectCard idCard game =
    let card =
        (getCard idCard game.cards)
        openedCard =
            {card | isDiscover = True}
        op = 
            List.append game.player.openedCards [openedCard]
        thePlayer =
            game.player
        nPlayer =
            {thePlayer | openedCards = op}
    in (
        let nGame =
            ({game | player = nPlayer})
        in (
            if List.length nGame.player.openedCards == 2 then 
                checkCards nGame
            else nGame
        )
    )
            

getCardWithIndex : Int -> Array.Array Card -> Card
getCardWithIndex index openedCards =
    let card = 
        Array.get index openedCards
    in (
        case card of
            Nothing -> initCard "00"
            Just val -> val
    )

isTheCardRev : Card -> String -> Bool
isTheCardRev card cardId =
    card.cardId == cardId

doOpenCard : List String -> Card -> Card 
doOpenCard cardIds card =
    if (List.any (isTheCardRev card) cardIds) then
        ({card | isDiscover = True})
    else card

openCards : List String -> Game -> Game
openCards cardIds game =
    let nCards =
        List.map (doOpenCard cardIds) game.cards
    in (
        {game | cards = nCards}
    )


checkMatch : String -> String -> Game -> Game 
checkMatch fId sId game =
    let player =
        game.player
    in (
        if (String.contains fId sId) then
            openCards [fId, sId] ({game | player = ({player | marks = player.marks + 1, openedCards = []})})
        else
            ({game | player = ({player | openedCards = []})})
    )
checkCards : Game -> Game
checkCards game =
    let player =
            game.player
        openedCards =
            Array.fromList player.openedCards
        fCardId =
            (getCardWithIndex 0 openedCards).cardId
        sCardId =
            (getCardWithIndex 1 openedCards).cardId
    in (
        if (String.length fCardId == String.length sCardId) then
            let nplayer =
                ({player | openedCards = []})
            in
                ({game | player = nplayer})
        else if (String.length fCardId < String.length sCardId) then
            checkMatch fCardId sCardId game
        else
            checkMatch sCardId fCardId game
    )


isTheCard : String -> Card -> Bool
isTheCard id card =
    if card.cardId == id then True else False

getCard : String -> List Card -> Card
getCard idCard cards =
    let card =
        List.head (List.filter (isTheCard idCard) cards)
    in (
        case card of
            Nothing -> (initCard idCard)
            Just val -> val
    )

-- VIEW

getLink : Card -> String
getLink card =
    if card.isDiscover then card.cardId
    else "cover"

linkToCard : List Card -> Card -> String
linkToCard openedCards card =
    if ((List.length openedCards) > 0) then
    (
        if card.isDiscover then card.cardId
        else (
            let foundCard =
                getCard card.cardId openedCards
            in (
                getLink foundCard
            )
        )
    )
    else
    (
        getLink card
    )

cardToHtml : List Card -> Card -> Html Msg
cardToHtml openedCards card =
    img[
    style[("width", "116px"), ("height", "160px")]
    , src ("./cards/" ++ (linkToCard openedCards card) ++ ".png")
    , onClick (CardId card.cardId)] []

printAllCards : List Card -> List Card -> Html Msg
printAllCards gameCards openedCards =
    div[style[("width", "464px"), ("margin", "auto")]]
        (List.map (cardToHtml openedCards) gameCards)

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
        , printAllCards game.cards game.player.openedCards
        ])