namespace Common

type Envelop(content) =
    member __.Content with get() = content
    member __.ContentType with get() = content.GetType().Name

//let New(content) = Envelop(content)



