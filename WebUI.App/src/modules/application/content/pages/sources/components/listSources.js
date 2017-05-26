import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route } from 'react-router-dom';

import { sourceControls } from './../../../data';
import ListSourcesItem from './../../../controls/components/items/listSourcesItem';

let ListSources = ({ match, sourceControls }) => (
    <Switch>
        <Route exact path={match.url} render={ () =>
            <div className="container">
                { sourceControls.data.map((sc) => {
                        return <ListSourcesItem key={sc.id} sourceControl={sc} />;
                })}
            </div>
        } />
    </Switch>
);

ListSources = componentDidMount(
    (props) => {
    }
)(ListSources);

const mapStateToProps = (state) => {
    return {
        sourceControls: state[sourceControls.constants.NAME]
    }
};

export default connect(mapStateToProps)(ListSources);
