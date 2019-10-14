import axios from "axios";

export const getMatches = (type, index) =>
    axios.get(`matches?filter=${type}&index=${index}`);

export const deleteMatch = (matchId) =>
    axios.delete(`/matches/${matchId}`);

export const joinMatch = (matchId) =>
    axios.put(`/matches/${matchId}`);

export const createMatch = (form) =>
    axios.post("/matches", form);