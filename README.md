# GorillaStats
A Gorilla Tag mod that shows useful information on a watch!<br/>
now allowing to add custom pages!!<br/>
[Join my discord for custom pages](https://discord.gg/4QQuYJu46e)<br/>
[![Github All Releases](https://img.shields.io/github/downloads/arielthemonke/GorillaStats/total.svg)]()

![Picture of the watch i think](watch/Watch.png)

## For developers:
### Custom pages:

I wanted to be resource friendly so I didnt add any automatic page adding, you will need to register your pages manually like this:
```c#
void Init()
{
    GorillaStatsPageManager.RegisterPage(new yourPageClass());
}
```

you will also need to create a seperate class for the page
```C#
public class yourPageClass : IGorillaStatsPage
{
    public string PageName => "yourPageName";
    
    public string GetPageText()
    {
        return "hi";
    }
}
```

### using the built in notification lib:
very easy. to send a notification just use this method:
```C#
NotifLib.instance.SendNotification("Your message here", 2f);
```
`2f` represents message duration in seconds

I think thats all. if you have any problems feel free to ask :)


> This product is not affiliated with Another Axiom Inc. or its videogames Gorilla Tag and Orion Drift and is not endorsed or otherwise sponsored by Another Axiom. Portions of the materials contained herein are property of Another Axiom. ©2021 Another Axiom Inc.
