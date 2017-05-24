import * as t from './actionTypes';

export const load = (sources) => ({
    type: t.LOAD,
    payload: sources
});