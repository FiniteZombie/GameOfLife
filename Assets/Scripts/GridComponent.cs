using UnityEngine;
using UnityEngine.UI;

public class GridComponent : MonoBehaviour
{
    public Image Image;
    public int Size;
    public Button TickButton;
    public Button ResetButton;
    public Button PlayButton;
    public InputField SizeInput;
    public Slider RateSlider;

    private readonly CellGrid _grid = new CellGrid();
    private Texture2D _texture;
    private bool _isPlaying = false;
    private float _nextTick;

    void Awake ()
	{
		_grid.Seed("seed_input.txt");
        TickButton.onClick.AddListener(OnTick);
	    ResetButton.onClick.AddListener(OnReset);
        PlayButton.onClick.AddListener(OnPlayPause);
        SizeInput.onEndEdit.AddListener(OnSizeInput);
        _nextTick = Time.time;
        Setup(Size);
        SizeInput.text = Size.ToString();
    }

    void Update()
    {
        if (_isPlaying && _nextTick < Time.time)
        {
            _nextTick = Time.time + RateSlider.value;
            Tick();
        }
    }

    private void Setup(int width)
    {
        _texture = new Texture2D(width, width);
        Image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(.5f, .5f));
        Image.preserveAspect = true;

        _texture.filterMode = FilterMode.Point;
        UpdateView();
    }

    private void Tick() {
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

    private void OnTick() {
        _isPlaying = false;
        PlayButton.GetComponentInChildren<Text>().text = "Play";
        Tick();
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

    private void OnPlayPause() 
    {
        _isPlaying = !_isPlaying;

        PlayButton.GetComponentInChildren<Text>().text = _isPlaying
            ? "Pause"
            : "Play";
    }
}
