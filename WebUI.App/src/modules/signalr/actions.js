import * as t from './actionTypes'

export const send = (payload) => ({
    type: t.SEND,
    payload: payload
});