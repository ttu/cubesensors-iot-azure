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

let WrapToNumber(content) =
    let item = { NumberItem.Value = content.ToString(); NumberItem.Text = DateTime.Now.ToShortTimeString() }
    { NumberItemContent.Item = [|item|] }
