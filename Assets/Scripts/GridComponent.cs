using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the cell grid, seeds it with an input file, and handles its graphical representation via pixels on a texture.
/// </summary>
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

    /// <summary>
    /// Unity Awake. Seeds cell input, registers listeners, and initializes the grid.
    /// </summary>
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

    /// <summary>
    /// Unity update. While in play mode, the simulation is ticked at a specified cadence
    /// </summary>
    void Update()
    {
        if (_isPlaying && _nextTick < Time.time)
        {
            _nextTick = Time.time + RateSlider.value;
            Tick();
        }
    }

    /// <summary>
    /// Initializes the view with a given size in pixels
    /// </summary>
    private void Setup(int width)
    {
        _texture = new Texture2D(width, width);
        Image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(.5f, .5f));
        Image.preserveAspect = true;

        _texture.filterMode = FilterMode.Point;
        UpdateView();
    }

    /// <summary>
    /// Tick the simulation and update the view
    /// </summary>
    private void Tick() {
        _grid.Tick();
        UpdateView();
    }

    /// <summary>
    /// Update the view via a texture on the linked image. Each pixel of the texture corresponds to a cell in the grid.
    /// </summary>
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

    /// <summary>
    /// Called when the tick button is pressed. Tick the simulation.
    /// </summary>
    private void OnTick() {
        _isPlaying = false;
        PlayButton.GetComponentInChildren<Text>().text = "Play";
        Tick();
    }

    /// <summary>
    /// Called when the size input field is updated. Reinitialize the grid view to the given size
    /// </summary>
    private void OnSizeInput(string sizeStr)
    {
        int n;
        if (int.TryParse(sizeStr, out n))
        {
            Size = n;
            Setup(n);
        }
    }

    /// <summary>
    /// Called when the reset button is pressed. Reset the grid to the seed
    /// </summary>
    private void OnReset()
    {
        _grid.Seed("seed_input.txt");
        UpdateView();
    }

    /// <summary>
    /// Called when the play/pause button is pressed. Toggle auto-ticking the simulation on/off
    /// </summary>
    private void OnPlayPause() 
    {
        _isPlaying = !_isPlaying;

        PlayButton.GetComponentInChildren<Text>().text = _isPlaying
            ? "Pause"
            : "Play";
    }
}
