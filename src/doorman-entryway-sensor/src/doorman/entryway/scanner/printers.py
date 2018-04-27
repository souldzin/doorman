def print_array(arr):
    print(", ".join((str(x) for x in arr)))


def print_row(row):
    print("---------------------")
    print_array((round(x, 2) for x in row))
    print("---------------------")

def print_matrix(cells, sel=None):
    if sel is None:
        sel = lambda x: x

    print("---------------------")
    for row in cells:
        print_array([(sel(x) if x is not None else "-") for x in row])
    print("---------------------")