export const UPLOAD_STATUS = {
    FAILED: -1,
    NOT_STARTED: 0,
    INITIAL_STARTED: 1,
    INITIAL_FINISHED: 2,
    TRIM_STARTED: 3,
    TRIM_FINISHED: 4,
    UPDATE_STARTED: 5,
};

export const MATCH_MODE = {
    ONE_UP: 'One Up',
    THREE_ROUND_PASS: 'Three Round Pass',
    COPY_CAT: 'Copy Cat',
};

export const MATCH_TYPES = {
    HOSTED: 'hosted',
    OPEN: 'open',
    HISTORY: 'history',
    ACTIVE: 'active',
    SPECTATE: 'spectate',
    NONE: '',
};

export const NOTIFICATION_TYPE = {
    WEB: 1,
    EMAIL: 2
};

export const LAYOUT = {
    LOADING: 'loading-layout',
    VISITOR: 'visitor-layout',
    USER: 'user-layout',
};

export const STORAGE_KEYS = {
    NOTIFICATION_PROMPT: 'tr_notification_prompt_time'  
};