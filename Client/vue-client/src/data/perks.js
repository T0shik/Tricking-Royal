import {mdiDoorOpen, mdiScale, mdiViewGridPlus} from "@mdi/js";

const perks = [
    {
        id: 0,
        name: "Host",
        icon: mdiViewGridPlus,
        description: "Increase the amount of matches you can host by 1.",
        current: (profile) => `Current hosting limit: ${profile.hostingLimit}`
    },
    {
        id: 1,
        name: "Guest",
        icon: mdiDoorOpen,
        description: "Increase the amount of match you can join by 1.",
        current: (profile) => `Current join limit: ${profile.joinedLimit}`
    },
    {
        id: 2,
        name: "Voting Power",
        icon: mdiScale,
        description: "Improve your voting power in the tribunal by 1.",
        current: (profile) => `Current voting power: ${profile.votingPower}`
    },
    // {
    //     id: 4,
    //     name: "Advertise",
    //     icon: mdiAccountCardDetails,
    //     description: "Advertise a website link of your choice."
    // }
];

export default perks;