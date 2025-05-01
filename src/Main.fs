module Main

open Feliz
open Browser.Dom
open Thoth.Json

[<ReactComponent>]
let MoodApp () =
    let now = System.DateTime.Now
    let defaultMood = {|
        Rating = 3
        Note = ""
        Timestamp = now
    |}

    let (mood, setMood) = React.useState defaultMood
    let (entries, setEntries) = React.useState([])

    React.useEffectOnce(fun () ->
        match window.localStorage.getItem("moodEntries") with
        | null -> ()
        | savedJson ->
            try
                let parsed = Decode.Auto.unsafeFromString<ResizeArray<_>> savedJson
                setEntries(List.ofSeq parsed)
            with _ -> ()
    )

    let saveEntry () =
        if mood.Rating > 0 || mood.Note <> "" then
            let newEntry = {| mood with Timestamp = System.DateTime.Now |}
            let updated = newEntry :: entries
            setEntries(updated)
            setMood({| defaultMood with Timestamp = System.DateTime.Now |})
            window.localStorage.setItem("moodEntries", Encode.Auto.toString(2, updated))

    Html.div [
        prop.style [
            style.fontFamily "Segoe UI, sans-serif"
            style.backgroundColor "#f1f5f9"
            style.padding 30
            style.borderRadius 12
            style.boxShadow(0, 0, 16, "rgba(0,0,0,0.1)")
            style.maxWidth 620
            style.margin.auto
            style.marginTop 40
        ]

        prop.children [

            Html.h1 [
                prop.text "Moodify 😊"
                prop.style [
                    style.textAlign.center
                    style.color "#2d3748"
                    style.marginBottom 20
                ]
            ]

            Html.p [
                prop.text "Hangulat értékelése (1–5):"
                prop.style [ style.fontWeight.bold ]
            ]

            Html.div [
                prop.style [ style.display.flex; style.marginBottom 20 ]
                prop.children [
                    for i in 1..5 ->
                        Html.button [
                            prop.text (string i)
                            prop.onClick (fun _ -> setMood({| mood with Rating = i |}))
                            prop.style [
                                style.marginRight 10
                                style.padding 10
                                style.backgroundColor (if i = mood.Rating then "#63b3ed" else "#e2e8f0")
                                style.borderRadius 6
                                style.cursor.pointer
                                style.width 40
                            ]
                        ]
                ]
            ]

            Html.textarea [
                prop.placeholder "Miért érzed így?"
                prop.value mood.Note
                prop.onChange (fun text -> setMood({| mood with Note = text |}))
                prop.rows 3
                prop.style [
                    style.width (length.percent 100)
                    style.marginBottom 20
                    style.padding 10
                    style.borderRadius 4
                    style.borderWidth 1
                    style.borderStyle.solid
                    style.borderColor "#cbd5e0"
                    style.fontSize 16
                ]
            ]

            Html.button [
                prop.text "💾 Mentés"
                prop.onClick (fun _ -> saveEntry ())
                prop.style [
                    style.padding 10
                    style.borderRadius 4
                    style.backgroundColor "#48bb78"
                    style.color.white
                    style.cursor.pointer
                    style.marginBottom 20
                ]
            ]

            Html.hr [ prop.style [ style.marginTop 30; style.marginBottom 20 ] ]

            Html.h2 [
                prop.text "Korábbi hangulatok:"
                prop.style [ style.color "#2d3748" ]
            ]

            Html.ul [
                for m in entries ->
                    Html.li [
                        prop.text (sprintf "%d/5 – %s (%s)" m.Rating m.Note (m.Timestamp.ToString("g")))
                        prop.style [ style.marginBottom 10 ]
                    ]
            ]
        ]
    ]

// Entry point
ReactDOM.createRoot(document.getElementById "feliz-app").render(MoodApp())