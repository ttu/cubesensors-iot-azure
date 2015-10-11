module Gecko

open System

// List Type
//[
//  {
//    "title": {
//      "text": "Chrome"
//    },
//    "label": {
//      "name": "New!",
//      "color": "#ff2015"
//    },
//    "description": "40327 visits"
//  },

// Number Type
//{
//  "item": [
//    {
//      "value": 5723,
//      "text": "Total paying customers"
//    }
//  ]
//}

type NumberItem = {
    Value : string;
    Text : string;
}

type NumberItemContent = {
    Item : NumberItem[]
}

type ListTitle = {
    Text : string;
}

type ListLabel = {
    Name : string;
    Color : string;
}

type ListItem = {
    Title : ListTitle;
    Label : ListLabel;
    Description : string;
}

let WrapToNumber(content, text) =
    let item = { NumberItem.Value = content; NumberItem.Text = text }
    { NumberItemContent.Item = [|item|] }

let WrapToList(items) =
    items
        |> Seq.map (fun x -> match x with 
                                | (title, desc, label, color) -> { 
                                                                    ListItem.Title = { ListTitle.Text = title }; 
                                                                    ListItem.Label = { ListLabel.Color = color; ListLabel.Name = label }; 
                                                                    ListItem.Description = desc
                                                                 })
