class Zadanie:
    def __init__(self, prio, naz):
        self.name = naz
        self.priority = prio
        self.progress = 0.

    def pro(self):
        return self.priority*(1-self.progress)

    def __add__(self, other):
        return self.pro() + other.pro()

    def __radd__(self, other):
        return self.pro() + other

CZAS = 120

def alg(zad, zid, zpro):
    suma = sum(zad)
    global CZAS
    obl = lambda x, s: x.pro()*CZAS/suma
    k = []
    times = [obl(z, suma) for z in zad]
    for z in zad:
        k.append("{0:8} - czas {1}".format(z.name, obl(z, suma)))
    CZAS = CZAS - times[zid]*zpro
    zad[zid].progress = zpro
    return k

def dump(l):
    for e in l:
        print(e)
    
zadania = [Zadanie(1, "matma"), Zadanie(1, "tv"), Zadanie(1, "poczta")]

print("{}min".format(CZAS))
lsa = alg(zadania, -1, 0.5)
dump(lsa)
print("-----------poczta w polowei")
print("{}min".format(CZAS))
lsb = alg(zadania, 1, 1)
dump(lsb)
print("-----------gotowa tv")
print("{}min".format(CZAS))
lsc = alg(zadania, -1, 1)
dump(lsc)