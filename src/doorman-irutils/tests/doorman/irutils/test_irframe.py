import doorman.irutils.irframe as irframe

def test_create_frame_from_points():
    points = [(4,4)]
    frame = irframe.create_frame_from_points(points)
    expected = [
        20, 20, 20, 20, 20, 20, 20, 20,
        20, 20, 20, 20, 20, 20, 20, 20,
        20, 20, 34, 34, 34, 34, 34, 20,
        20, 20, 34, 38, 38, 38, 34, 20,
        20, 20, 34, 38, 38, 38, 34, 20,
        20, 20, 34, 34, 34, 34, 34, 20,
        20, 20, 20, 20, 20, 20, 20, 20,
        20, 20, 20, 20, 20, 20, 20, 20,
    ]
    assert(frame == expected)