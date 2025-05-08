using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireApp;

public class HighlightSelectorManager
{

    private IList<Image> _yellowSelectorImages = new List<Image>();
    private int _yellowSelectorsInUse = 0;

    public Image GetYellowSelector() 
    {
        if (_yellowSelectorsInUse >= _yellowSelectorImages.Count) 
        {
            var yellowSelector = CreateNewYellowSelector();
            _yellowSelectorImages.Add(yellowSelector);
        }

        _yellowSelectorsInUse++;
        return _yellowSelectorImages[_yellowSelectorsInUse-1];
    }
    public void ClearYellowSelectors(AbsoluteLayout layout) 
    {
        foreach (Image selector in _yellowSelectorImages) 
        {
            layout.Children.Remove(selector);
        }
        _yellowSelectorsInUse = 0;
    }

    private Image CreateNewYellowSelector()
    {
        return new Image
        {
            AutomationId = Guid.NewGuid().ToString(),
            Source = "card_overlay_yellow.png",
        };
    }

    public Image CreateGreenSelector()
    {
        return new Image
        {
            AutomationId = "GreenSelector",
            Source = "card_overlay_green.png",
        };
    }

    public Image CreateRedSelector()
    {
        return new Image
        {
            AutomationId = "RedSelector",
            Source = "card_overlay_red.png",
        };
    }
}
