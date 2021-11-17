# Tower of Hanoi
[Play it now!](http://rafael.se/games/tower-of-hanoi/)

## Thoughts and decisions

### Physics for fun
Besides personally being immensely attracted to the use of physics in any possible way, I actually thought this was a most suitable project to approach using 2D mechanical simulation, using the advised graphics. And seeing the disks wobble, stumble and stack, immediately reassured me in my conviction. I especially liked how disk holes, being slightly wider, allowed the disks to wiggle a bit on their way up and down the pegs, and also to pile up a bit uncentered. This, together with the different faces, naturally suggested the idea to have a disk make a slight shake, when being grabbed while not allowed, as if shaking its head.

### Restrictions spur creativity
As for the one-rule implementation, my first idea was to have a larger disk bounce back when trying to enter a peg with a smaller disk, and return to its original peg while the small would shake in the same way as when being illicitly grabbed. While playing around and tweaking the motions, it struck me that it could be fun if when the larger disk entered the peg, the smaller disk would jump up and bounce it off, rebounding it to the peg it came from. Up until then my scripts was fairly clean, and realizing the idea caused quite a creative mess in the code kitchen that I'm still trying to tidy. But I think it was worth it. I find it playful somehow, and don't think it would be perceived as offensive by children.

### Target audience testing
My own kids, 5 and 8 years of age at the time, played early on, as soon as it was possible to move disks and place them on pegs, but in a rough and sketchy stage with no restrictions. They were captivated right away, as they usually are with anything involving moving characters on a screen. It was fascinating to see how strictly they both followed the rules that I'd briefly explained, even though there was nothing to stop them from putting larger disks on smaller ones. And also how they patiently awaited the at that time still very spinning disks, in order to have the right side up before placing them on pegs. I had expected them to be amused by throwing disks around, lifting up many at once, watching them fall over, placing them upside down and so on. I'm amazed and surprised every day. I've probably learned more from children than from adults in my life.

### Time flies
I found this an extraordinarily amusing project. I've been completely immersed, and would love to keep going. I focused on the feel and experience above all, rather than cleanliness and consideration for the code reader. Given more time, I would refactor quite a lot, dividing the code into smaller and more manageable pieces. I would put more effort into naming and commenting things. I would of course adapt it for touch devices. I would do a lot more. I would make it perfect.
