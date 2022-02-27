import React from 'react';
import clsx from 'clsx';
import styles from './styles.module.css';
import Link from '@docusaurus/Link';

const FeatureList = [
  {
    title: 'Start Quickly',
  Svg: require('@site/static/img/undraw_docusaurus_mountain.svg').default,
    description: (
      <>
        Add a few lines to your Docker Compose file and you're ready to go.
      </>
    ),
  },
  {
    title: 'gRPC Support',
    Svg: require('@site/static/img/undraw_docusaurus_tree.svg').default,
    description: (
      <>
        We reccomend using gRPC. It's easy to get started and
        supports all of the features of the HTTP API. For more information,
        read the <Link to="/docs/api-versions/grpc">gRPC documentation</Link>.
      </>
    ),
  },
  {
    title: 'Powered by C#',
    Svg: require('@site/static/img/undraw_docusaurus_react.svg').default,
    description: (
      <>
        Primarily dependent on <Link to="https://github.com/Tyrrrz/DiscordChatExporter">Tyrrrz/DiscordChatExporter</Link>,
        we vendor the C# codebase to ensure a performant and reliable experience.
      </>
    ),
  },
];

function Feature({Svg, title, description}) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
        <Svg className={styles.featureSvg} alt={title} />
      </div>
      <div className="text--center padding-horiz--md">
        <h3>{title}</h3>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
