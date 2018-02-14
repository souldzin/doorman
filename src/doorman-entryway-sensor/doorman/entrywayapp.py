#!/usr/bin/env python
import doorman.irutils.fake_camera as camera

def main():
    print("Here we go!")
    print(camera.get_frame())

if __name__ == '__main__':
    main()
