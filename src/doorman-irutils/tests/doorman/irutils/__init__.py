import doorman.irutils.irframe as irframe

def test_create_frame_from_points():
    frame = irframe.create_frame_from_points([(4,4)])
    print(frame)