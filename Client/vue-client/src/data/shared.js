export const surfaces = [
    "any", "sprungFloor", "grass", "concrete", "trampoline", "tumbling"
];

export const skills =
    [
        {name: 'beginner', value: 0},
        {name: 'intermediate', value: 1},
        {name: 'skilled', value: 2},
        {name: 'expert', value: 3},
        {name: 'master', value: 4},
        {name: 'goat', value: 5}
    ];

export const matches = [
    {
        value: 0,
        rules: [0, 1, 2, 3, 4, 5],
        turnTypes: null
    },
    {
        value: 1,
        rules: [0, 1],
        turnTypes: [0, 1, 2]
    },
    {
        value: 2,
        rules: [0, 1, 2, 3],
        turnTypes: null
    }
];