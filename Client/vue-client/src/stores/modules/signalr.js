import {HubConnectionBuilder, LogLevel} from "@aspnet/signalr"
import Logger from "../../logger/logger";

export default {
    namespaced: true,
    state: {
        connection: null
    },
    mutations: {
        setConnection(state, {connection}) {
            state.connection = connection;
        }
    },
    actions: {
        initConnection({commit, rootGetters, dispatch}) {
            let connection = new HubConnectionBuilder()
                .withUrl(process.env.VUE_APP_API + '/hub/match-updater',
                    {
                        accessTokenFactory: () => {
                            return rootGetters.GET_ACCESS_TOKEN;
                        }
                    }
                )
                .configureLogging(process.env.NODE_ENV === 'production' ? LogLevel.Critical : LogLevel.Information)
                .build();


            connection.on("MatchUpdateFinished", (payload) => {
                Logger.log("[signalr.js] MatchUpdateFinished payload:", payload);
                let notification = {
                    message: "Match Updated",
                    success: true
                };
                dispatch("DISPLAY_POPUP_DEFAULT", notification, {root: true});
                dispatch("matches/refreshMatches", {}, {root: true})
            });

            connection.start()
                .catch(function (err) {
                    Logger.error(err.toString());
                });

            commit("setConnection", {connection});
        }
    }
}

