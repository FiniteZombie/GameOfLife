using UnityEngine;
using UnityEngine.UI;

public class GridComponent : MonoBehaviour
{
    public Image Image;
    public int Size;
    public Button TickButton;
    public Button ResizeButton;
    public Button ResetButton;
    public InputField SizeInput;

    private readonly CellGrid _grid = new CellGrid();
    private Texture2D _texture;

    void Awake ()
	{
		_grid.Seed("seed_input.txt");
        TickButton.onClick.AddListener(OnTick);
        ResizeButton.onClick.AddListener(OnResize);
	    ResetButton.onClick.AddListener(OnReset);
        SizeInput.onEndEdit.AddListener(OnSizeInput);
	}

    void Start()
    {
        Setup(Size);
    }

    private void Setup(int width)
    {
        _texture = new Texture2D(width, width);
        Image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(.5f, .5f));
        Image.preserveAspect = true;

        _texture.filterMode = FilterMode.Point;
        UpdateView();
    }

    private void OnTick()
    {
        _grid.Tick();
        UpdateView();
    }

    private void UpdateView()
    {
        var centerX = _texture.width / 2;
        var centerY = _texture.height / 2;
        for (var y = 0; y < _texture.height; y++)
        {
            for (var x = 0; x < _texture.width; x++)
            {
                //var color = ((x & y) != 0 ? Color.white : Color.gray);
                var color = _grid.IsAlive(x - centerX, y - centerY) ? Color.white : Color.gray;
                _texture.SetPixel(x, y, color);
            }
        }

        _texture.Apply();
    }

    private void OnResize()
    {
        Setup(Size);
    }

    private void OnSizeInput(string arg0)
    {
        int n;
        if (int.TryParse(arg0, out n))
        {
            Size = n;
            Setup(n);
        }
    }

    private void OnReset()
    {
        _grid.Seed("seed_input.txt");
        UpdateView();
    }
}
