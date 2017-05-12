import * as t from './actionTypes'

export const change = (shortName) => ({
    type: t.CHANGE,
    payload: shortName
});
