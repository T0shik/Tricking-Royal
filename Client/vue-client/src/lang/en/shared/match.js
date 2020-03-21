export default {
    modes: [
        {
            name: "One Up",
            description: "Try to one up your opponent by adding a trick to the end of the combo.",
            rules: [
                "One trick per turn.",
                "Setup moves count as tricks.",
                "Transitions don't count as tricks.",
                "You have to repeat tricks as is.",
                "These battles don't get judged.",
                "If you cannot advance the combo you have to forfeit."
            ]
        },
        {
            name: "Three Round Pass",
            description: "Competition standard, take turns to upload combos, after 3 passes are uploaded match is taken to the tribunal where the winner is decided by other users.",
            rules: [
                "You are allowed to re-use videos from other battles & social media.",
                "One combo per video, please film horizontally."
            ]
        },
        {
            name: "Copy Cat",
            description: "Take turns to copy each others combos, gain a point for each combo. Tricker with the most points wins.",
            rules: [
                "You have to repeat the combo as is.",
                "If you can't repeat the combo you have to pass the round.",
                "When you pass you don't gain a point.",
                "Maximum of 4 rounds, user with the most points wins."
            ]
        },
    ],
    turnTypes: ["Blitz", "Classic", "Alternating"],
    surfaces: ["Any", "Spring Floor", "Grass", "Concrete", "Trampoline", "Tumbling"]
}
